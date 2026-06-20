import { HttpEvent, HttpInterceptorFn, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../services/busy-service';
import { delay, finalize, of, tap } from 'rxjs';

const cach = new Map<string, HttpEvent<unknown>>();

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  const generateCacheKey = (url: string, params: HttpParams): string => {
    const paramsString = params
      .keys()
      .map((key) => `${key}=${params.get(key)}`)
      .join('&');
    return paramsString ? `${url}?${paramsString}` : url;
  };

  const cacheKey = generateCacheKey(req.url, req.params);

  if (req.method === 'GET') {
    const cachedResponse = cach.get(cacheKey);
    if (cachedResponse) {
      return of(cachedResponse);
    }
  }

  busyService.busy();

  return next(req).pipe(
    delay(500),
    tap((resp) => {
      cach.set(cacheKey, resp);
    }),
    finalize(() => {
      busyService.idle();
    }),
  );
};
