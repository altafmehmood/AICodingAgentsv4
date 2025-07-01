import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Breach {
  name: string;
  title: string;
  domain: string;
  breachDate: string;
  pwnCount: number;
  description: string;
  dataClasses: string[];
  isVerified: boolean;
  isFabricated: boolean;
  isSensitive: boolean;
  isRetired: boolean;
  isSpamList: boolean;
  logoPath: string;
}

@Injectable({
  providedIn: 'root',
})
export class BreachService {
  private readonly apiUrl = 'https://localhost:5001/breaches';

  private http = inject(HttpClient);

  getBreachByName(name: string): Observable<Breach> {
    return this.http.get<Breach>(`${this.apiUrl}/${name}`);
  }

  getRiskAnalysis(description: string): Observable<string> {
    return this.http.post(
      `${this.apiUrl}/analyze`,
      { description },
      { responseType: 'text' },
    );
  }

  generateReport(breach: Breach, riskAnalysis: string): Observable<Blob> {
    return this.http.post(
      `${this.apiUrl}/report`,
      { breach, riskAnalysis },
      { responseType: 'blob' },
    );
  }

  getBreachesByEmail(email: string): Observable<Breach[]> {
    return this.http.get<Breach[]>(`${this.apiUrl}/email/${email}`);
  }
}
