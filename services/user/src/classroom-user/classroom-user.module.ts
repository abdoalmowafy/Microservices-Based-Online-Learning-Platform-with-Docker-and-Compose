import { Module } from '@nestjs/common';
import { ClassroomUserService } from './classroom-user.service';
import { ClassroomUserResolver } from './classroom-user.resolver';
import { PrismaModule } from 'src/prisma/prisma.module';

@Module({
  imports: [PrismaModule],
  providers: [ClassroomUserService, ClassroomUserResolver]
})
export class ClassroomUserModule { }
