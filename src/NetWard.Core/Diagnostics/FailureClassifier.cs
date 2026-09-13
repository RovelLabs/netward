using NetWard.Core.Models;

namespace NetWard.Core.Diagnostics;

public interface IFailureClassifier
{
    ServiceHealthVerdict Classify(ServiceProfile profile, List<LayerProbeResult> layers);
}

public class FailureClassifier : IFailureClassifier
{
    public ServiceHealthVerdict Classify(ServiceProfile profile, List<LayerProbeResult> layers)
    {
        var verdict = new ServiceHealthVerdict
        {
            ServiceId = profile.Id,
            DisplayName = profile.DisplayName,
            Category = profile.Category,
            LayerResults = layers,
            LastChecked = DateTime.UtcNow
        };

        var dns = layers.FirstOrDefault(l => l.Layer == ProbeLayer.Dns);
        var tcp = layers.FirstOrDefault(l => l.Layer == ProbeLayer.TcpHandshake);
        var tls = layers.FirstOrDefault(l => l.Layer == ProbeLayer.TlsHandshake);
        var http = layers.FirstOrDefault(l => l.Layer == ProbeLayer.HttpLayer);
        var stream = layers.FirstOrDefault(l => l.Layer == ProbeLayer.ContentStream);

        // 1. Check if completely offline / local gateway down
        if (dns != null && !dns.IsSuccess && tcp != null && !tcp.IsSuccess && tls != null && !tls.IsSuccess)
        {
            verdict.Status = ServiceStatus.LocalIssue;
            verdict.FailureCause = FailureCause.LocalGatewayFailure;
            verdict.ConfidencePercent = 90;
            verdict.SummaryRu = "Проблема с локальным подключением";
            verdict.SummaryEn = "Local network connection issue";
            verdict.TechnicalDetailsRu = "Не удалось разрешить DNS и установить ни одного TCP-соединения. Похоже, отсутствует интернет-соединение.";
            verdict.TechnicalDetailsEn = "DNS resolution and all TCP handshakes failed. Internet connection appears down.";
            verdict.RecommendationRu = "Проверьте кабель интернета или статус подключения к Wi-Fi роутеру.";
            verdict.RecommendationEn = "Check your Ethernet cable or Wi-Fi router connection status.";
            return verdict;
        }

        // 2. Check for DNS Poisoning
        if (dns != null && dns.ResolvedIps.Any(ip => ip.StartsWith("127.") || ip == "0.0.0.0"))
        {
            verdict.Status = ServiceStatus.Blocked;
            verdict.FailureCause = FailureCause.DnsPoisoned;
            verdict.ConfidencePercent = 95;
            verdict.SummaryRu = "DNS-подмена (блокировка на уровне провайдера)";
            verdict.SummaryEn = "DNS poisoning (ISP-level block)";
            verdict.TechnicalDetailsRu = "DNS-сервер провайдера вернул локальный адрес 127.0.0.1 вместо настоящего IP сервиса.";
            verdict.TechnicalDetailsEn = "ISP DNS resolver returned localhost 127.0.0.1 instead of real server addresses.";
            verdict.RecommendationRu = "Использование безопасного DNS (DoH) может восстановить корректное определение адресов.";
            verdict.RecommendationEn = "Configuring an encrypted DNS resolver (DoH) may restore valid name resolution.";
            return verdict;
        }

        // 3. Check for TSPU SNI Filtering (TCP SYN succeeds, but TLS ClientHello fails with RST 10054)
        if (tcp != null && tcp.IsSuccess && tls != null && !tls.IsSuccess)
        {
            bool isRst = tls.SocketErrorCode == 10054 ||
                         (tls.Details != null && tls.Details.Contains("RST", StringComparison.OrdinalIgnoreCase)) ||
                         (tls.ErrorMessage != null && tls.ErrorMessage.Contains("forcibly closed", StringComparison.OrdinalIgnoreCase));

            verdict.Status = ServiceStatus.Blocked;
            verdict.FailureCause = isRst ? FailureCause.TspuSniBlock : FailureCause.TlsInterception;
            verdict.ConfidencePercent = isRst ? 96 : 85;
            verdict.OverallLatencyMs = tcp.LatencyMs;

            if (isRst)
            {
                verdict.SummaryRu = "Сетевая блокировка ТСПУ (сброс соединения по SNI)";
                verdict.SummaryEn = "TSPU filtering active (TCP RST during SNI handshake)";
                verdict.TechnicalDetailsRu = $"TCP-соединение на порт 443 успешно установлено ({tcp.LatencyMs:F0} мс), но при отправке TLS ClientHello с именем сервиса фильтр ТСПУ провайдера принудительно разорвал сессию пакетом TCP RST.";
                verdict.TechnicalDetailsEn = $"TCP handshake on port 443 succeeded ({tcp.LatencyMs:F0} ms), but the ISP's TSPU filter injected a TCP RST packet immediately upon receiving TLS ClientHello SNI.";
                verdict.RecommendationRu = "Проблема не на вашем компьютере и не в Wi-Fi роутере. Перезагрузка оборудования не поможет.";
                verdict.RecommendationEn = "The issue is not on your computer or Wi-Fi router. Restarting equipment will not resolve it.";
            }
            else
            {
                verdict.SummaryRu = "Ошибка согласования защищённого соединения (TLS)";
                verdict.SummaryEn = "Secure connection handshake failure (TLS)";
                verdict.TechnicalDetailsRu = $"Не удалось завершить TLS-рукопожатие: {tls.ErrorMessage}";
                verdict.TechnicalDetailsEn = $"TLS handshake failed: {tls.ErrorMessage}";
                verdict.RecommendationRu = "Возможно вмешательство прокси или антивирусного сканера в защищённый трафик.";
                verdict.RecommendationEn = "Possible local proxy or antivirus scanning interference in secure traffic.";
            }
            return verdict;
        }

        // 4. Check for ISP Block Page (HTTP 451)
        if (http != null && http.HttpStatusCode == 451)
        {
            verdict.Status = ServiceStatus.Blocked;
            verdict.FailureCause = FailureCause.IspBlockPage;
            verdict.ConfidencePercent = 99;
            verdict.SummaryRu = "Блокировка по требованию регулятора (HTTP 451)";
            verdict.SummaryEn = "Blocked by regulator demand (HTTP 451)";
            verdict.TechnicalDetailsRu = "Сервер или провайдер вернул официальный статус '451 Unavailable For Legal Reasons'.";
            verdict.TechnicalDetailsEn = "Server or ISP returned HTTP '451 Unavailable For Legal Reasons'.";
            verdict.RecommendationRu = "Доступ ограничен на основании законодательства.";
            verdict.RecommendationEn = "Access restricted under applicable regulations.";
            return verdict;
        }

        // 5. Check for Global Cloud Server Outage (500, 502, 503)
        if (http != null && http.HttpStatusCode.HasValue && http.HttpStatusCode.Value >= 500)
        {
            verdict.Status = ServiceStatus.ServerOutage;
            verdict.FailureCause = FailureCause.GlobalServerOutage;
            verdict.ConfidencePercent = 92;
            verdict.OverallLatencyMs = http.LatencyMs;
            verdict.SummaryRu = "Глобальный сбой на серверах сервиса";
            verdict.SummaryEn = "Global outage on service servers";
            verdict.TechnicalDetailsRu = $"Сервер ответил кодом HTTP {http.HttpStatusCode.Value}. Сбой на стороне владельцев сервиса, а не вашего провайдера.";
            verdict.TechnicalDetailsEn = $"Remote server returned HTTP {http.HttpStatusCode.Value}. This is an issue with the service itself, not your ISP.";
            verdict.RecommendationRu = "Подождите, пока инженеры сервиса устранят неисправность.";
            verdict.RecommendationEn = "Wait for service engineers to restore server infrastructure.";
            return verdict;
        }

        // 6. Check for Provider-Side Geo-Restriction (HTTP 403 with specific services like OpenAI)
        if (http != null && http.HttpStatusCode == 403 && profile.Id.Equals("openai", StringComparison.OrdinalIgnoreCase))
        {
            verdict.Status = ServiceStatus.Blocked;
            verdict.FailureCause = FailureCause.GeoRestricted;
            verdict.ConfidencePercent = 95;
            verdict.SummaryRu = "Региональное ограничение со стороны сервиса (Geo-block)";
            verdict.SummaryEn = "Service provider geo-restriction (Cloudflare 403)";
            verdict.TechnicalDetailsRu = "Сервер OpenAI вернул код 403 Forbidden: сервис самостоятельно ограничивает доступ для российских IP-адресов.";
            verdict.TechnicalDetailsEn = "OpenAI edge returned HTTP 403: service restricts access from Russian IP subnets.";
            verdict.RecommendationRu = "Ограничение введено со стороны сервиса OpenAI, а не российскими операторами связи.";
            verdict.RecommendationEn = "Restriction is applied on the service provider side, not by local ISPs.";
            return verdict;
        }

        // 7. Check for Traffic Throttling (YouTube GGC throttling)
        if (stream != null && !stream.IsSuccess && stream.ThroughputKbps.HasValue && stream.ThroughputKbps.Value < 250.0)
        {
            verdict.Status = ServiceStatus.Throttled;
            verdict.FailureCause = FailureCause.ThrottledBandwidth;
            verdict.ConfidencePercent = 94;
            verdict.OverallLatencyMs = stream.LatencyMs;
            verdict.SummaryRu = "Искусственное замедление скорости (Throttling)";
            verdict.SummaryEn = "Artificial traffic throttling detected";
            verdict.TechnicalDetailsRu = $"Скорость загрузки видеопотока снижена до {stream.ThroughputKbps.Value:F0} кбит/с из-за избирательного сброса пакетов на узлах CDN.";
            verdict.TechnicalDetailsEn = $"Video stream throughput is policed to {stream.ThroughputKbps.Value:F0} kbps due to selective packet dropping on CDN nodes.";
            verdict.RecommendationRu = "Соединение работает, но видео в высоком качестве будет постоянно буферизоваться.";
            verdict.RecommendationEn = "Connection establishes, but high-resolution video will buffer continuously.";
            return verdict;
        }

        // 8. Normal / Healthy
        double avgLatency = (tcp?.LatencyMs ?? 50.0 + (tls?.LatencyMs ?? 50.0)) / 2.0;
        verdict.OverallLatencyMs = Math.Round(avgLatency, 1);

        if (avgLatency > 180.0)
        {
            verdict.Status = ServiceStatus.Degraded;
            verdict.FailureCause = FailureCause.None;
            verdict.ConfidencePercent = 85;
            verdict.SummaryRu = "Высокая задержка (высокий пинг)";
            verdict.SummaryEn = "High connection latency (elevated ping)";
            verdict.TechnicalDetailsRu = $"Задержка отклика составляет {avgLatency:F0} мс. Возможна загрузка внешних магистральных каналов провайдера.";
            verdict.TechnicalDetailsEn = $"Round-trip latency is {avgLatency:F0} ms. Upstream ISP transit routes may be congested.";
            verdict.RecommendationRu = "Сервис работает, но возможны задержки при загрузке данных.";
            verdict.RecommendationEn = "Service is operational, but high response times may be noticeable.";
        }
        else
        {
            verdict.Status = ServiceStatus.Healthy;
            verdict.FailureCause = FailureCause.None;
            verdict.ConfidencePercent = 98;
            verdict.SummaryRu = "Работает стабильно";
            verdict.SummaryEn = "Operating normally";
            verdict.TechnicalDetailsRu = $"Все сетевые уровни пройдены успешно (DNS, TCP {tcp?.LatencyMs:F0} мс, TLS {tls?.LatencyMs:F0} мс). Блокировок не обнаружено.";
            verdict.TechnicalDetailsEn = $"All network layers verified (DNS, TCP {tcp?.LatencyMs:F0} ms, TLS {tls?.LatencyMs:F0} ms). No filtering detected.";
            verdict.RecommendationRu = "Никаких действий не требуется, сервис полностью доступен.";
            verdict.RecommendationEn = "No action required, service is fully accessible.";
        }

        return verdict;
    }
}
