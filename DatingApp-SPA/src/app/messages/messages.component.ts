import { Component, OnInit } from '@angular/core';
import { Pagination } from '../_models/Pagination';
import { Message } from '../_models/Message';
import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';

@Component({
	selector: 'app-messages',
	templateUrl: './messages.component.html',
	styleUrls: [ './messages.component.css' ]
})
export class MessagesComponent implements OnInit {
	messages: Message[];
	pagination: Pagination;
	messageContainer = 'Unread';

	constructor(
		private userService: UserService,
		private authService: AuthService,
		private route: ActivatedRoute,
		private alertify: AlertifyService
	) {}

	ngOnInit() {
		this.route.data.subscribe((data) => {
			this.messages = data['messages'].result;
			this.pagination = data['messages'].pagination;
		});
	}

	loadMessages() {
		this.messages = null;
		this.userService
			.getMessages(
				this.authService.decodedToken.nameid,
				this.pagination.currentPage,
				this.pagination.itemsPerPage,
				this.messageContainer
			)
			.subscribe(
				(res) => {
					this.messages = res.result;
					this.pagination = res.pagination;
					console.log(res);
				},
				(error) => {
					this.alertify.error(error);
				}
			);
	}

	deleteMessage(id: number) {
		console.log(id);
		this.alertify.confirm('Are you sure you want to delete this message', () => {
			this.userService.deleteMessage(id, this.authService.decodedToken.nameid).subscribe(
				() => {
					this.messages.splice(this.messages.findIndex((m) => m.id === id), 1);
					this.alertify.success('Message has been deleted');
				},
				(error) => {
					this.alertify.error('Failed to delete the message');
				}
			);
		});
	}

	pageChanged(event: any) {
		this.pagination.currentPage = event.page;
		this.loadMessages();
	}
}
