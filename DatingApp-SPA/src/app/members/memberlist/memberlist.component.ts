import { Component, OnInit } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { User } from '../../_models/user';
import { ActivatedRoute } from '@angular/router';

@Component({
	selector: 'app-memberlist',
	templateUrl: './memberlist.component.html',
	styleUrls: [ './memberlist.component.css' ]
})
export class MemberlistComponent implements OnInit {
	users: User[];

	constructor(private userService: UserService, private alertify: AlertifyService, private route: ActivatedRoute) {}

	ngOnInit() {
		this.route.data.subscribe((data) => {
			console.log(data);
			this.users = data['users'];
		});
	}

	loadUsers() {
		this.userService.getUsers().subscribe(
			(users: User[]) => {
				this.users = users;
			},
			(error) => {
				this.alertify.error(error);
			}
		);
	}
}
