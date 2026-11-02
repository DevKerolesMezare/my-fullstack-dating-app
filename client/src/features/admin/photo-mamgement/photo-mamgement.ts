import { Component, inject, signal } from '@angular/core';
import { AdminService } from '../../../core/services/admin-service';
import { Photo } from '../../../types/member';

@Component({
  selector: 'app-photo-mamgement',
  imports: [],
  templateUrl: './photo-mamgement.html',
  styleUrl: './photo-mamgement.css',
})
export class PhotoMamgement {
  photos = signal<Photo[]>([]);
  private adminService = inject(AdminService);

  ngOnInit(): void {
    this.getPhotosForApproval();
  }

  getPhotosForApproval() {
    this.adminService.getPhotosForApproval().subscribe({
      next: (photos) => this.photos.set(photos),
    });
  }

  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe({
      next: () =>
        this.photos.update((photos) => {
          return photos.filter((x) => x.id !== photoId);
        }),
    });
  }

  rejectPhoto(photoId: number) {
    this.adminService.rejectPhoto(photoId).subscribe({
      next: () =>
        this.photos.update((photos) => {
          return photos.filter((x) => x.id !== photoId);
        }),
    });
  }
}
