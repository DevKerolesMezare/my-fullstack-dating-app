import { ResolveFn, Router } from '@angular/router';
import { MemberService } from '../../core/services/member-service';
import { inject } from '@angular/core/primitives/di';
import { EMPTY } from 'rxjs';
import { Member } from '../../types/member';

export const memberResolver: ResolveFn<Member> = (route) => {
  const memberService = inject(MemberService);
  const router = inject(Router);
  const memvberId = route.paramMap.get('id');

  if (!memvberId) {
    router.navigate(['/not-found']);
    return EMPTY;
  }

  return memberService.getMember(memvberId);
};
