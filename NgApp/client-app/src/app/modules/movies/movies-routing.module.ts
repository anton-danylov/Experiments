import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { MovieComponent } from './components/movie/movie.component';
import { MoviesListComponent } from './components/movies-list/movies-list.component';


const moviesRoutes: Routes = [
  { path: 'movies', component: MoviesListComponent },
  { path: 'mov', component: MoviesListComponent },
];


@NgModule({
  imports: [RouterModule.forChild(moviesRoutes)],
  exports: [RouterModule],
})

export class MoviesRoutingModule { }
