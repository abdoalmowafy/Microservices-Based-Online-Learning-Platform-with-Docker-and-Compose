import { Injectable } from '@nestjs/common';
import { PassportStrategy } from '@nestjs/passport';
import { ExtractJwt, Strategy } from 'passport-jwt';
import { CurrentUserModel } from './current-user.model';

const KEY = process.env.JwtConfig_Key;
if (!KEY) throw new Error('JwtConfig_Key is not defined');

const ISSUER = process.env.JwtConfig_Issuer;
if (!ISSUER) throw new Error('JwtConfig_Issuer is not defined');

const AUDIENCE = process.env.JwtConfig_Audience;
if (!AUDIENCE) throw new Error('JwtConfig_Audience is not defined');

@Injectable()
export class JwtStrategy extends PassportStrategy(Strategy) {
    constructor() {
        super({
            jwtFromRequest: ExtractJwt.fromAuthHeaderAsBearerToken(),
            // ignoreExpiration: false,
            ignoreExpiration: true,
            secretOrKey: KEY!,
            issuer: ISSUER,
            audience: AUDIENCE,
        });
    }

    validate(payload: any): CurrentUserModel {
        return {
            id: payload.sub,
            role: payload.role,
        };
    }
}
