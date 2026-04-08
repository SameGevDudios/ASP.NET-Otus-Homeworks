import { useState } from 'react';
import { Container, Paper, TextField, Button, Typography, Box } from '@mui/material';
import { useNavigate, Link } from 'react-router-dom';
import { useAppDispatch } from '@/app/store/hooks';
import { login } from '@/features/auth/authSlice';

export const LoginPage = () => {
  const [name, setName] = useState('');
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const handleLogin = () => {
    if (name.trim()) {
      dispatch(login(name));
      navigate('/');
    }
  };

  return (
    <Container maxWidth="xs">
      <Box sx={{ mt: 15 }}>
        <Paper sx={{ p: 4 }}>
          <Typography variant="h5" align="center" gutterBottom>FitTrack Login</Typography>
          <TextField 
            fullWidth 
            label="Enter your name" 
            variant="outlined" 
            margin="normal"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
          <Button 
            fullWidth 
            variant="contained" 
            size="large" 
            sx={{ mt: 2 }}
            onClick={handleLogin}
          >
            Sign In
          </Button>
          <Typography align="center" sx={{ mt: 2 }}>
            New here? <Link to="/register">Register</Link>
          </Typography>
        </Paper>
      </Box>
    </Container>
  );
};