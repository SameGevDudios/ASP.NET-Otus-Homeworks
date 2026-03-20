import { Container, Paper, TextField, Button, Typography, Box } from '@mui/material';
import { Link } from 'react-router-dom';

export const RegisterPage = () => {
  return (
    <Container maxWidth="xs">
      <Box sx={{ mt: 10 }}>
        <Paper sx={{ p: 4 }}>
          <Typography variant="h5" align="center" gutterBottom>Create Account</Typography>
          <TextField fullWidth label="Full Name" margin="normal" />
          <TextField fullWidth label="Email" margin="normal" />
          <TextField fullWidth label="Password" type="password" margin="normal" />
          <Button fullWidth variant="contained" color="success" sx={{ mt: 2 }}>Join FitTrack</Button>
          <Typography align="center" sx={{ mt: 2 }}>
            Have an account? <Link to="/login">Login</Link>
          </Typography>
        </Paper>
      </Box>
    </Container>
  );
};