import { Module } from '@nestjs/common';
import { ClassroomResolver } from './classroom.resolver';
import { ClassroomService } from './classroom.service';
import { PrismaModule } from 'src/prisma/prisma.module';

@Module({
  imports: [PrismaModule],
  providers: [ClassroomResolver, ClassroomService]
})
export class ClassroomModule { }
