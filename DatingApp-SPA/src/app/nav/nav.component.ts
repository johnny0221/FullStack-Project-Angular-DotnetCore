import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
	selector: 'app-nav',
	templateUrl: './nav.component.html',
	styleUrls: [ './nav.component.css' ]
})
export class NavComponent implements OnInit {
	constructor(public authService: AuthService, private alertify: AlertifyService, private router: Router) {}

	model: FormGroup;
	photoUrl: string;

	ngOnInit() {
		this.initForm();
		this.authService.currentPhotoUrl.subscribe((photoUrl) => {
			this.photoUrl = photoUrl;
		});
	}

	initForm() {
		this.model = new FormGroup({
			Username: new FormControl(null, Validators.required),
			Password: new FormControl(null, Validators.required)
		});
	}

	onSubmit() {
		const data = this.model.value;
		this.authService.login(data).subscribe(
			(next) => {
				this.alertify.success('successfully login');
				this.router.navigate([ '/members' ]);
			},
			(error) => {
				this.alertify.error(error);
			},
			() => {}
		);
	}

	isloggedIn() {
		return this.authService.isloggedIn();
	}

	logout() {
		localStorage.removeItem('token');
		localStorage.removeItem('user');
		this.authService.decodedToken = null;
		this.authService.currentUser = null;
		this.alertify.message('logout');
		this.router.navigate([ '/home' ]);
	}
}
