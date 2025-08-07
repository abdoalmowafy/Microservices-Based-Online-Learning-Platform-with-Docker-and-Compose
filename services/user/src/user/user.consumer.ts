import { RabbitSubscribe } from "@golevelup/nestjs-rabbitmq";
import { Injectable } from "@nestjs/common";
import { UserService } from "src/user/user.service";
import { UserCreateEvent } from "./events/incoming/user-create.event";
import { UserAvatarEvent } from "./events/incoming/user-avatar.event";
import { UserPublisher } from "./user.publisher";

@Injectable()
export class UserConsumer {
    constructor(private readonly userService: UserService, private readonly userPublisher: UserPublisher) { }

    @RabbitSubscribe({
        exchange: 'user',
        routingKey: 'user.created',
        queue: 'user.created',
    })
    async userCreated(payload: UserCreateEvent) {
        try {
            await this.userService.create(payload);
        } catch (error) { }
    }

    @RabbitSubscribe({
        exchange: 'user',
        routingKey: 'user.avatar',
        queue: 'user.avatar',
    })
    async userAvatar(payload: UserAvatarEvent) {
        try {
            await this.userService.avatar(payload.id, payload.url);
        } catch (error) {
            this.userPublisher.publishUserInfo(payload.id);
        }
    }
}