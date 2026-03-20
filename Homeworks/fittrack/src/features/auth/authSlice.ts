import { createSlice, type PayloadAction } from '@reduxjs/toolkit';

interface AuthState {
  isAuth: boolean;
  user: string | null;
  isLoading: boolean;
}

const initialState: AuthState = {
  isAuth: false,
  user: null,
  isLoading: false,
};

export const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    login: (state, action: PayloadAction<string>) => {
      state.isAuth = true;
      state.user = action.payload;
    },
    logout: (state) => {
      state.isAuth = false;
      state.user = null;
    },
  },
});

export const { login, logout } = authSlice.actions;
export default authSlice.reducer;