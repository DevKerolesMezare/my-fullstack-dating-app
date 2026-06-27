import { NgIf } from '@angular/common';
import { Component, inject } from '@angular/core';
import { AccountService } from '../../core/services/account-service';
import { UserManagement } from './user-management/user-management';
import { PhotoMamgement } from './photo-mamgement/photo-mamgement';

@Component({
  selector: 'app-admin',
  imports: [UserManagement , PhotoMamgement],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
})
export class Admin {
  protected accountService = inject(AccountService);
  activeTab = 'photos';
  tabs = [
    {label: 'Pthoto moderation', value: 'photos'},
    {label: 'User moderation', value: 'roles'},
  ]

  setTab(tab: string)
  {
    this.activeTab = tab;
  }
}
