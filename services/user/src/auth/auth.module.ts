import { Module } from '@nestjs/common';
import { JwtStrategy } from './jwt.strategy';
import { PassportModule } from '@nestjs/passport';
@Module({
    imports: [PassportModule],
    providers: [JwtStrategy],
    exports: [PassportModule],
})
export class AuthModule { }
