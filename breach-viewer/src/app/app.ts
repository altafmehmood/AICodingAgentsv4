import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { BreachService, Breach } from './breach.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  breachName = '';
  email = '';
  breach: Breach | null = null;
  breachesByEmail: Breach[] | null = null;
  riskAnalysis: string | null = null;
  loading = false;
  error: string | null = null;

  private breachService = inject(BreachService);

  search() {
    this.loading = true;
    this.error = null;
    this.breach = null;
    this.riskAnalysis = null;
    this.breachesByEmail = null;

    this.breachService.getBreachByName(this.breachName).subscribe(
      (breach) => {
        this.breach = breach;
        this.breachService.getRiskAnalysis(breach.description).subscribe(
          (riskAnalysis) => {
            this.riskAnalysis = riskAnalysis;
            this.loading = false;
          },
          () => {
            this.error = 'Error getting risk analysis.';
            this.loading = false;
          },
        );
      },
      () => {
        this.error = 'Breach not found.';
        this.loading = false;
      },
    );
  }

  searchByEmail() {
    this.loading = true;
    this.error = null;
    this.breach = null;
    this.riskAnalysis = null;
    this.breachesByEmail = null;

    this.breachService.getBreachesByEmail(this.email).subscribe(
      (breaches) => {
        this.breachesByEmail = breaches;
        this.loading = false;
      },
      () => {
        this.error = 'No breaches found for this email.';
        this.loading = false;
      },
    );
  }

  generateReport() {
    if (this.breach && this.riskAnalysis) {
      this.breachService
        .generateReport(this.breach, this.riskAnalysis)
        .subscribe((blob) => {
          const url = window.URL.createObjectURL(blob);
          window.open(url);
        });
    }
  }
}
