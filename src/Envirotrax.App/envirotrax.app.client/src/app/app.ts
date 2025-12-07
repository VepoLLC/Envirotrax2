import { Component, signal, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.css'
})
export class App implements OnInit {

  constructor() { }

  ngOnInit() {

  }

  protected readonly title = signal('envirotrax.app.client');
}
