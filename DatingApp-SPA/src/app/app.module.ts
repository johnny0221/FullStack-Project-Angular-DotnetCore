import { BrowserModule, HAMMER_GESTURE_CONFIG, HammerGestureConfig } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { JwtModule } from '@auth0/angular-jwt';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { MemberlistComponent } from './members/memberlist/memberlist.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { appRoutes } from './routes';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { NgxGalleryModule } from 'ngx-gallery';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './prevent-unsaved-changes.guard';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { FileUploadModule } from 'ng2-file-upload';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TimeAgoPipe } from 'time-ago-pipe';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';

export function tokenGetter() {
	return localStorage.getItem('token');
}

export class CustomHammerConfig extends HammerGestureConfig {
	overrides = {
		pinch: { enable: false },
		rotate: { enable: false }
	};
}

@NgModule({
	declarations: [
		AppComponent,
		NavComponent,
		HomeComponent,
		RegisterComponent,
		MemberlistComponent,
		ListsComponent,
		MessagesComponent,
		MemberCardComponent,
		MemberDetailComponent,
		MemberEditComponent,
		PhotoEditorComponent,
		TimeAgoPipe
	],
	imports: [
		BrowserModule,
		BrowserAnimationsModule,
		HttpClientModule,
		ReactiveFormsModule,
		FormsModule,
		BrowserAnimationsModule,
		BsDropdownModule.forRoot(),
		BsDatepickerModule.forRoot(),
		RouterModule.forRoot(appRoutes),
		JwtModule.forRoot({
			config: {
				tokenGetter,
				whitelistedDomains: [ 'localhost:5000' ],
				blacklistedRoutes: [ 'localhost:5000/api/auth' ]
			}
		}),
		TabsModule.forRoot(),
		NgxGalleryModule,
		FileUploadModule,
		PaginationModule.forRoot(),
		ButtonsModule.forRoot()
	],
	providers: [
		ErrorInterceptorProvider,
		MemberDetailResolver,
		MemberListResolver,
		MemberEditResolver,
		{ provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig },
		PreventUnsavedChanges
	],
	bootstrap: [ AppComponent ]
})
export class AppModule {}
