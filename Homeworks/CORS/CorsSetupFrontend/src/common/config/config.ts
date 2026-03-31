export class Config {
  private static readonly BASE_URL = import.meta.env.VITE_BASE_URL;

  static getBaseUrl(): string {
    this.validate();
    return this.BASE_URL;
  }

  private static validate(): void {
    if (!this.BASE_URL) {
      console.warn('VITE_BASE_URL is missing in .env');
    }
  }
}
