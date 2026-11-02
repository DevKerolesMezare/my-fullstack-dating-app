import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { ToastService } from './toast-service';
import { User } from '../../types/user';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { Message } from '../../types/messages';
import { Subject } from 'rxjs';

export type PhotoApprovedEvent = {
  photoId: number;
  mainImageUrl: string | null;
};

export type PhotoRejectedEvent = {
  photoId: number;
};

export type PhotoAwaitingApprovalEvent = {
  id: number;
  url: string;
  userId: string;
  username: string;
  isApproved: false;
};

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  private huUrl = environment.hubUrl;
  private toast = inject(ToastService);
  hubConnection?: HubConnection;
  onlineUsers = signal<string[]>([]);
  photoApproved$ = new Subject<PhotoApprovedEvent>();
  photoRejected$ = new Subject<PhotoRejectedEvent>();
  photoAwaitingApproval$ = new Subject<PhotoAwaitingApprovalEvent>();

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.huUrl + 'presence', {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log(error));

    this.hubConnection.on('UserOnline', (userId) => {
      this.onlineUsers.update((users) => [...users, userId]);
    });

    this.hubConnection.on('UserOffline', (userId) => {
      this.onlineUsers.update((users) => users.filter((x) => x !== userId));
    });

    this.hubConnection.on('GetOnlineUsers', (userIds) => {
      this.onlineUsers.set(userIds);
    });

    this.hubConnection.on('PhotoApproved', (event: PhotoApprovedEvent) => {
      this.photoApproved$.next(event);
    });

    this.hubConnection.on('PhotoRejected', (event: PhotoRejectedEvent) => {
      this.photoRejected$.next(event);
    });

    this.hubConnection.on(
      'PhotoAwaitingApproval',
      (photo: PhotoAwaitingApprovalEvent) => this.photoAwaitingApproval$.next(photo),
    );

    this.hubConnection.on('NewMessageReceived', (message: Message) => {
      this.toast.info(
        `You have a new message from ${message.senderDisplayName}`,
        10000,
        message.senderImageUrl,
        `/members/${message.senderId}/messages`,
      );
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch((error) => console.log(error));
    }
  }
}
