import { Box, Typography, Button } from '@mui/material';
import { useNavigate } from 'react-router-dom';

export const NotFoundPage = () => {
  const navigate = useNavigate();
  return (
    <Box sx={{ textAlign: 'center', mt: 20 }}>
      <Typography variant="h1" color="primary">404</Typography>
      <Typography variant="h5" sx={{ mb: 3 }}>Page Not Found</Typography>
      <Button variant="contained" onClick={() => navigate('/')}>Go Home</Button>
    </Box>
  );
};