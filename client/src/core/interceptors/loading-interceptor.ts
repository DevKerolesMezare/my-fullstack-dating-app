import { HttpEvent, HttpInterceptorFn, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../services/busy-service';
import { delay, finalize, identity, of, tap } from 'rxjs';
import { environment } from '../../environments/environment';

type CacheEntry = {
  response: HttpEvent<unknown>;
  timestamp: number;
};

const cache = new Map<string, CacheEntry>();
const CACHE_DURATION_MS = 5 * 60 * 1000;

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  const generateCacheKey = (url: string, params: HttpParams): string => {
    const paramsString = params
      .keys()
      .map((key) => `${key}=${params.get(key)}`)
      .join('&');
    return paramsString ? `${url}?${paramsString}` : url;
  };

  const invalidateCache = (urlPattern: string) => {
    for (const key of cache.keys()) {
      if (key.includes(urlPattern)) {
        cache.delete(key);
      }
    }
  };

  const cacheKey = generateCacheKey(req.url, req.params);

  if (req.method.includes('POST') && req.url.includes('/likes')) {
    invalidateCache('/likes');
  }

  if (req.method.includes('POST') && req.url.includes('/messages')) {
    invalidateCache('/messages');
  }

  if (req.method.includes('POST') && req.url.includes('/account')) {
    cache.clear();
  }

  if (req.method === 'GET') {
    const cachedEntry = cache.get(cacheKey);
    if (cachedEntry) {
      const isExpired = (Date.now() - cachedEntry.timestamp) > CACHE_DURATION_MS;
      if (!isExpired) {
        return of(cachedEntry.response);
      }
      cache.delete(cacheKey);
    }
  }

  busyService.busy();

  return next(req).pipe(
    environment.production ? identity : delay(1000),
    tap((resp) => {
      cache.set(cacheKey, { response: resp, timestamp: Date.now() });
    }),
    finalize(() => {
      busyService.idle();
    }),
  );
};
