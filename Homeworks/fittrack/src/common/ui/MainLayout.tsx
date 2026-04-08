import { AppBar, Toolbar, Typography, Button, Container, Box } from '@mui/material';
import FitnessCenterIcon from '@mui/icons-material/FitnessCenter';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch } from '@/app/store/hooks';
import { logout } from '@/features/auth/authSlice';

interface Props {
  children: React.ReactNode;
  user?: string | null;
}

export const MainLayout = ({ children, user }: Props) => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  return (
    <Box sx={{ flexGrow: 1, minHeight: '100vh', bgcolor: '#ffffff' }}>
      <AppBar position="static">
        <Toolbar>
          <FitnessCenterIcon sx={{ mr: 2 }} />
          <Typography variant="h6" sx={{ flexGrow: 1, cursor: 'pointer' }} onClick={() => navigate('/')}>
            FitTrack
          </Typography>
          {user && (
            <>
              <Button color="inherit" onClick={() => navigate('/')}>Dashboard</Button>
              <Button color="inherit" onClick={() => dispatch(logout())}>Logout</Button>
            </>
          )}
        </Toolbar>
      </AppBar>
      <Container sx={{ mt: 4 }}>{children}</Container>
    </Box>
  );
};