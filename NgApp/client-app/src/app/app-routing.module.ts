import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { CharactersListComponent } from './components/characters-list/characters-list.component';
import { PageNotFoundComponent } from './components/page-not-found.component';


const appRoutes: Routes = [
  { path: 'characters', component: CharactersListComponent },
  { path: '', redirectTo: 'movies', pathMatch: 'full' },
  { path: '**', component: PageNotFoundComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(appRoutes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
