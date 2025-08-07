import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import { ValidationPipe } from '@nestjs/common';

async function bootstrap() {
  const app = await NestFactory.create(AppModule);
  app.useGlobalPipes(new ValidationPipe({
    transform: true, // enables class-transformer
    whitelist: true, // strips unknown properties
    forbidNonWhitelisted: true, // optional: throw on unknown props
  }));
  await app.listen(process.env.UserService_PORT || 3000);
}
bootstrap();
