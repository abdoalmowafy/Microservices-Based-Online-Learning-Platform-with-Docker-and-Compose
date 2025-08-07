import { Module } from '@nestjs/common';
import { UserResolver } from './user.resolver';
import { UserService } from './user.service';
import { UserConsumer } from './user.consumer';
import { PrismaModule } from 'src/prisma/prisma.module';
import { UserPublisher } from './user.publisher';
import { SharedRabbitMQModule } from 'src/shared/rabbitmq.module';

@Module({
  imports: [PrismaModule, SharedRabbitMQModule],
  providers: [UserResolver, UserService, UserConsumer, UserPublisher],
})
export class UserModule { }
