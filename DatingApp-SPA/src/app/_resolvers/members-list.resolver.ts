import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';

import { Observable,  of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AlertifyService } from '../_services/alertify.service';

@Injectable()
export class MembersListResolver implements Resolve<User[]> {
    pageNumber =1;
    pageSize = 5;

    constructor(private userService: UserService,
                private router: Router, private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<User[]> {
        return this.userService.getUsers(this.pageNumber, this.pageSize).pipe(
            catchError(error => {
                this.alertify.error('Error retrieving data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }

}
