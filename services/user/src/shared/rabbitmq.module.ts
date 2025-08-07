import { RabbitMQModule } from "@golevelup/nestjs-rabbitmq";
import { Global, Module } from "@nestjs/common";

@Global()
@Module({
    imports: [
        RabbitMQModule.forRoot({
            uri: process.env.RABBITMQ_URL,
            connectionInitOptions: { wait: true },
            exchanges: [
                { name: 'user', type: 'topic' },
                { name: 'organization', type: 'topic' },
                { name: 'classroom', type: 'topic' },
            ],
        })
    ],
    exports: [RabbitMQModule]
})
export class SharedRabbitMQModule { }