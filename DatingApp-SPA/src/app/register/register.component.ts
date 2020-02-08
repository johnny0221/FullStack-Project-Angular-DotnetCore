import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
	selector: 'app-register',
	templateUrl: './register.component.html',
	styleUrls: [ './register.component.css' ]
})
export class RegisterComponent implements OnInit {
	constructor(private authService: AuthService, private alertify: AlertifyService) {}

	model: FormGroup;
	@Output() cancelRegister = new EventEmitter();

	ngOnInit() {
		this.initForm();
	}

	initForm() {
		this.model = new FormGroup({
			Username: new FormControl(null, Validators.required),
			Password: new FormControl(null, Validators.required)
		});
	}

	cancel() {
		// this.eventemitter.emit([values you want to emit])
		this.cancelRegister.emit(false);
	}

	register() {
		const data = this.model.value;
		this.authService.register(data).subscribe(
			(response) => {
				this.alertify.success('successfully registered');
			},
			(error) => {
				this.alertify.error(error);
			}
		);
	}
}
