import { ResolveFn } from '@angular/router';

export const memberResolverResolver: ResolveFn<boolean> = (route, state) => {
  return true;
};
