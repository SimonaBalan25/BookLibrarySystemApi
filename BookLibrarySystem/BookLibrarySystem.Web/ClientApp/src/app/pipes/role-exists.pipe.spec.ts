// role-exists.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'roleExists'
})
export class RoleExistsPipe implements PipeTransform {
  transform(roleToCheck: string, userRoles: string[]): boolean {
    return userRoles.includes(roleToCheck);
  }
}
