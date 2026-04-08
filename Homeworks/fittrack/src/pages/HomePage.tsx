import { Typography, Grid, Paper, Box } from '@mui/material';
import { withProtectedRoute } from '@/common/hoc/withProtectedRoute';
import { useAppSelector } from '@/app/store/hooks';

const HomePageBase = () => {
  const { user } = useAppSelector(state => state.auth);

  return (
    <Box>
      <Typography variant="h4" gutterBottom>Welcome back, {user}!</Typography>
      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 4 }}>
          <Paper sx={{ p: 2, textAlign: 'center', bgcolor: '#e3f2fd' }}>
            <Typography variant="h6">Calories</Typography>
            <Typography variant="h4">1,240</Typography>
          </Paper>
        </Grid>
        
        <Grid size={{ xs: 12, md: 4 }}>
          <Paper sx={{ p: 2, textAlign: 'center', bgcolor: '#f1f8e9' }}>
            <Typography variant="h6">Steps</Typography>
            <Typography variant="h4">8,432</Typography>
          </Paper>
        </Grid>
        
        <Grid size={{ xs: 12, md: 4 }}>
          <Paper sx={{ p: 2, textAlign: 'center', bgcolor: '#fff3e0' }}>
            <Typography variant="h6">Workouts</Typography>
            <Typography variant="h4">12</Typography>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};

export const HomePage = withProtectedRoute(HomePageBase);