import { Injectable } from '@nestjs/common';
import { IJoinCrudService } from 'src/shared/Ijoin-crud.service';
import { ClassroomCourse } from '@prisma/client';
import { ClassroomCourseCreateInput } from './inputs/classroom-course-create.input';
import { PrismaService } from 'src/prisma/prisma.service';

@Injectable()
export class ClassroomCourseService implements IJoinCrudService<ClassroomCourse> {
    constructor(private readonly prisma: PrismaService) { }

    findAll(): Promise<ClassroomCourse[]> {
        return this.prisma.classroomCourse.findMany();
    }

    findById(classroomId: string, courseId: string): Promise<ClassroomCourse | null> {
        return this.prisma.classroomCourse.findUnique({ where: { classroomId_courseId: { classroomId, courseId } } });
    }

    create(data: ClassroomCourseCreateInput): Promise<ClassroomCourse> {
        return this.prisma.classroomCourse.create({ data });
    }

    update(id1: string, id2: string, data: undefined): Promise<ClassroomCourse> {
        throw new Error('Method not implemented.');
    }

    delete(id1: string, id2: string): Promise<ClassroomCourse> {
        return this.prisma.classroomCourse.update({ where: { classroomId_courseId: { classroomId: id1, courseId: id2 } }, data: { removedAt: new Date() } });
    }
}
