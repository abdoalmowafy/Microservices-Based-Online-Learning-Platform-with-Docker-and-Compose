import { Module } from '@nestjs/common';
import { ClassroomCourseService } from './classroom-course.service';
import { PrismaModule } from 'src/prisma/prisma.module';

@Module({
  imports: [PrismaModule],
  providers: [ClassroomCourseService]
})
export class ClassroomCourseModule { }
