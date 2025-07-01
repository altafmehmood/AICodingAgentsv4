import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { BreachService, Breach } from './breach.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  breachName: string = '';
  breach: Breach | null = null;
  riskAnalysis: string | null = null;
  loading: boolean = false;
  error: string | null = null;

  constructor(private breachService: BreachService) { }

  search() {
    this.loading = true;
    this.error = null;
    this.breach = null;
    this.riskAnalysis = null;

    this.breachService.getBreachByName(this.breachName).subscribe(
      (breach) => {
        this.breach = breach;
        this.breachService.getRiskAnalysis(breach.description).subscribe(
          (riskAnalysis) => {
            this.riskAnalysis = riskAnalysis;
            this.loading = false;
          },
          (error) => {
            this.error = 'Error getting risk analysis.';
            this.loading = false;
          }
        );
      },
      (error) => {
        this.error = 'Breach not found.';
        this.loading = false;
      }
    );
  }

  generateReport() {
    if (this.breach && this.riskAnalysis) {
      this.breachService.generateReport(this.breach, this.riskAnalysis).subscribe((blob) => {
        const url = window.URL.createObjectURL(blob);
        window.open(url);
      });
    }
  }
}
