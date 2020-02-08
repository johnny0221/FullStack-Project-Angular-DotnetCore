import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/_models/Photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from '../../../environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
	selector: 'app-photo-editor',
	templateUrl: './photo-editor.component.html',
	styleUrls: [ './photo-editor.component.css' ]
})
export class PhotoEditorComponent implements OnInit {
	@Input() photos: Photo[];
	@Output() getMemberPhotoChange = new EventEmitter<string>();
	uploader: FileUploader;
	hasBaseDropZoneOver = false;
	baseUrl = environment.apiUrl;
	currentMain: Photo;

	constructor(
		private authService: AuthService,
		private userService: UserService,
		private alertify: AlertifyService
	) {}

	ngOnInit() {
		this.initUploader();
	}

	setMainPhoto(photo: Photo) {
		this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(
			(next) => {
				this.currentMain = this.photos.filter((p) => p.isMain === true)[0];
				this.currentMain.isMain = false;
				photo.isMain = true;
				this.authService.changeMemberPhoto(photo.url);
				this.authService.currentUser.photoUrl = photo.url;
				localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
			},
			(error) => {
				this.alertify.error(error);
			}
		);
	}

	deletePhoto(id: number) {
		this.alertify.confirm('Are you sure to delete the photo', () => {
			this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(
				() => {
					this.photos.splice(this.photos.findIndex((p) => p.id === id), 1);
					this.alertify.success('Photo has been deleted');
				},
				(error) => {
					this.alertify.error('failed to delete the photo');
				}
			);
		});
	}

	// logic for third party image upload.

	fileOverBase(e: any) {
		this.hasBaseDropZoneOver = e;
	}

	// just think of this as the configuration of pressing the upload button, it will send it to the post route of your server.
	initUploader() {
		this.uploader = new FileUploader({
			url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
			authToken: 'Bearer ' + localStorage.getItem('token'),
			allowedFileType: [ 'image' ],
			removeAfterUpload: true,
			autoUpload: false,
			maxFileSize: 10 * 1024 * 1024
		});

		this.uploader.onAfterAddingFile = (file) => {
			file.withCredentials = false;
		};

		// you can think of this function subscribes to a success response of the server.
		this.uploader.onSuccessItem = (item, response, status, headers) => {
			if (response) {
				const res: Photo = JSON.parse(response);
				const photo = {
					id: res.id,
					url: res.url,
					dateAdded: res.dateAdded,
					description: res.description,
					isMain: res.isMain
				};
				this.photos.push(photo);
			}
		};
	}
}
