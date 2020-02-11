import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker/public_api';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
	selector: 'app-register',
	templateUrl: './register.component.html',
	styleUrls: [ './register.component.css' ]
})
export class RegisterComponent implements OnInit {
	user: User;
	model: FormGroup;
	bsConfig: Partial<BsDatepickerConfig>;
	@Output() cancelRegister = new EventEmitter();

	constructor(
		private authService: AuthService,
		private alertify: AlertifyService,
		private fb: FormBuilder,
		private router: Router
	) {}

	ngOnInit() {
		this.bsConfig = {
			containerClass: 'theme-red'
		};
		this.initForm();
	}

	initForm() {
		this.model = this.fb.group(
			{
				Gender: [ 'male' ],
				Username: [ '', Validators.required ],
				Password: [ '', [ Validators.required, Validators.minLength(4), Validators.maxLength(8) ] ],
				KnownAs: [ '', Validators.required ],
				DateOfBirth: [ null, Validators.required ],
				City: [ '', Validators.required ],
				Country: [ '', Validators.required ],
				Confirmpassword: [ '', Validators.required ]
			},
			{ validators: this.passwordMatchValidator }
		);
	}

	passwordMatchValidator(f: FormGroup) {
		return f.get('Password').value === f.get('Confirmpassword').value ? null : { mismatched: true };
	}

	cancel() {
		// this.eventemitter.emit([values you want to emit])
		this.cancelRegister.emit(false);
	}

	register() {
		if (this.model.valid) {
			this.user = Object.assign({}, this.model.value);
			this.authService.register(this.user).subscribe(
				() => {
					this.alertify.success('Registration Successful');
				},
				(error) => {
					this.alertify.error(error);
				},
				() => {
					this.authService.login(this.user).subscribe(() => {
						this.router.navigate([ '/members' ]);
					});
				}
			);
		}
	}
}
