import { AmqpConnection } from "@golevelup/nestjs-rabbitmq";
import { Injectable } from "@nestjs/common";

@Injectable()
export class UserPublisher {
    constructor(private readonly amqpConnection: AmqpConnection) { }

    async publishUserInfo(userId: string) {
        await this.amqpConnection.publish("user", "user.Info", { Id: userId });
    }
}