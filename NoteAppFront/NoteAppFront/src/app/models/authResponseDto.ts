export interface AuthResponseDto {
  isAuthSuccessful: boolean;
  isVerified: boolean;
  token: string;
  userId: string;
}
