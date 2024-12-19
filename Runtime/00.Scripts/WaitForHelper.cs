using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Hian.Utilities
{
    public static class WaitForHelper
    {
        private static readonly ConcurrentDictionary<
            float,
            (WaitForSeconds wait, float lastUsedTime)
        > _waitForSecondsCache = new ConcurrentDictionary<float, (WaitForSeconds, float)>();

        private static readonly ConcurrentDictionary<
            float,
            (WaitForSecondsRealtime wait, float lastUsedTime)
        > _waitForSecondsRealtimeCache =
            new ConcurrentDictionary<float, (WaitForSecondsRealtime, float)>();

        private static readonly CancellationTokenSource _cleanupCancellationTokenSource;
        private static readonly Task _cleanupTask;

        private const float DefaultUnusedTimeThreshold = 60f;
        private const float DefaultCleanupInterval = 60f;

        static WaitForHelper()
        {
            _cleanupCancellationTokenSource = new CancellationTokenSource();
            _cleanupTask = Task.Run(
                () =>
                    CleanupUnusedCacheAsync(
                        DefaultUnusedTimeThreshold,
                        DefaultCleanupInterval,
                        _cleanupCancellationTokenSource.Token
                    )
            );

            // 애플리케이션 종료 시 Task 취소 및 리소스 해제
            Application.quitting += () =>
            {
                _cleanupCancellationTokenSource?.Cancel();
                _cleanupTask?.Wait();
                _cleanupCancellationTokenSource?.Dispose();
                _cleanupTask?.Dispose();
            };
        }

        /// <summary>
        /// 캐싱된 WaitForSeconds 객체를 반환합니다.
        /// </summary>
        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            var cached = _waitForSecondsCache.GetOrAdd(
                seconds,
                s => (new WaitForSeconds(s), Time.time)
            );
            _waitForSecondsCache[seconds] = (cached.wait, Time.time); // 사용 시간 갱신
            return cached.wait;
        }

        /// <summary>
        /// 캐싱된 WaitForSecondsRealtime 객체를 반환합니다.
        /// </summary>
        public static WaitForSecondsRealtime GetWaitForSecondsRealtime(float seconds)
        {
            if (Time.timeScale == 0)
            {
                // 일시 정지 상태에서는 항상 새로운 객체 반환
                return new WaitForSecondsRealtime(seconds);
            }

            var cached = _waitForSecondsRealtimeCache.GetOrAdd(
                seconds,
                s => (new WaitForSecondsRealtime(s), Time.realtimeSinceStartup)
            );
            _waitForSecondsRealtimeCache[seconds] = (cached.wait, Time.realtimeSinceStartup); // 사용 시간 갱신
            return cached.wait;
        }

        /// <summary>
        /// 일정 시간 동안 사용되지 않은 캐시를 비동기적으로 정리합니다.
        /// </summary>
        /// <param name="unusedTimeThreshold">캐시 유지 시간 (초)</param>
        /// <param name="cleanupInterval">캐시 정리 주기 (초)</param>
        /// <param name="cancellationToken">Task 취소 토큰</param>
        private static async Task CleanupUnusedCacheAsync(
            float unusedTimeThreshold,
            float cleanupInterval,
            CancellationToken cancellationToken
        )
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                CleanupUnusedCache(unusedTimeThreshold);

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(cleanupInterval), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 일정 시간 동안 사용되지 않은 캐시를 정리합니다.
        /// </summary>
        /// <param name="unusedTimeThreshold">캐시 유지 시간 (초)</param>
        private static void CleanupUnusedCache(float unusedTimeThreshold)
        {
            var currentTime = Time.time;
            foreach (var kvp in _waitForSecondsCache)
            {
                if (
                    currentTime - kvp.Value.lastUsedTime > unusedTimeThreshold
                    && _waitForSecondsCache.ContainsKey(kvp.Key)
                )
                {
                    _waitForSecondsCache.Remove(kvp.Key, out _);
                }
            }

            var currentRealtime = Time.realtimeSinceStartup;
            foreach (var kvp in _waitForSecondsRealtimeCache)
            {
                if (
                    currentRealtime - kvp.Value.lastUsedTime > unusedTimeThreshold
                    && _waitForSecondsRealtimeCache.ContainsKey(kvp.Key)
                )
                {
                    _waitForSecondsRealtimeCache.Remove(kvp.Key, out _);
                }
            }
        }
    }
}
