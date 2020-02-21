import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/Message';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';

// This component is a child component of the member detail component

@Component({
	selector: 'app-member-messages',
	templateUrl: './member-messages.component.html',
	styleUrls: [ './member-messages.component.css' ]
})
export class MemberMessagesComponent implements OnInit {
	@Input() recipientId: number; // fetched from the User property in the MemberDetail component.
	messages: Message[];

	constructor(
		private userService: UserService,
		private authService: AuthService,
		private alertify: AlertifyService
	) {}

	ngOnInit() {
		this.loadMessages();
	}

	loadMessages() {
		this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId).subscribe(
			(messages) => {
				this.messages = messages;
				console.log(messages);
			},
			(error) => {
				this.alertify.error(error);
			}
		);
	}
}
