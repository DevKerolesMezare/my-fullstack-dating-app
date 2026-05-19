import { HttpClient } from '@angular/common/http';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  private httpclient = inject(HttpClient);
  private destroyRef = inject(DestroyRef)
  protected readonly title = 'My-Dating-App';
  protected members =  signal<any>([]);

  ngOnInit(): void {
    const sub = this.httpclient.get('https://localhost:5001/api/members').subscribe({
      next: val =>this.members.set(val),
      error: (error) => console.log(error),
      complete: () => console.log('Completed'),
    });
  
    this.destroyRef.onDestroy(()=>sub.unsubscribe());
  }
}
