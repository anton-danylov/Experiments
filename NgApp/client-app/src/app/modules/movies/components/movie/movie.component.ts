import { Component, OnInit, Input, PipeTransform } from '@angular/core';
import { DatePipe  } from '@angular/common';
import { Movie } from '../../classes/movie';

@Component({
  selector: 'app-movie',
  templateUrl: './movie.component.html',
  styleUrls: ['./movie.component.css']
})
export class MovieComponent implements OnInit {

  @Input()
  movie = Movie;

  constructor() { }

  ngOnInit() {
  }

}
