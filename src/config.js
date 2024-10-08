const env = process.env.NODE_ENV || 'development';

const config = {
  development: {
    API_URL: 'http://localhost:5004/api',
    // other development-specific configurations
  },
  production: {
    API_URL: 'https://owapp-f3aua3bzcea2hudc.ukwest-01.azurewebsites.net/api',
    // other production-specific configurations
  }
};

export default config[env];