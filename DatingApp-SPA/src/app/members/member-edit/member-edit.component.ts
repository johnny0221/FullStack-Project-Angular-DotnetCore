import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators, NgForm } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Subscription, fromEvent } from 'rxjs';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
	selector: 'app-member-edit',
	templateUrl: './member-edit.component.html',
	styleUrls: [ './member-edit.component.css' ]
})
export class MemberEditComponent implements OnInit {
	@ViewChild('editForm', { static: true })
	editForm: NgForm;
	user: User;
	photoUrl: string;

	constructor(
		private route: ActivatedRoute,
		private alertify: AlertifyService,
		private userService: UserService,
		private authService: AuthService
	) {}

	ngOnInit() {
		this.route.data.subscribe((data) => {
			this.user = data['user'];
		});
		this.authService.currentPhotoUrl.subscribe((photoUrl) => {
			this.photoUrl = photoUrl;
		});
	}

	updateUser() {
		this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(
			(next) => {
				this.alertify.success('Profile has been updated successfully');
				this.editForm.reset(this.user);
			},
			(error) => {
				this.alertify.error(error);
			}
		);
	}

	updateMainPhoto(photoUrl: string) {
		this.user.photoUrl = photoUrl;
	}
}
