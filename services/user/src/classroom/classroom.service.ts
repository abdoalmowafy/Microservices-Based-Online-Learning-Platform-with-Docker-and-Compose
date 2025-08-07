import { Injectable } from '@nestjs/common';
import { IBaseCrudService } from 'src/shared/Ibase-crud.service';
import { Classroom } from '@prisma/client';
import { PrismaService } from 'src/prisma/prisma.service';
import { ClassroomCreateInput } from './inputs/classroom-create.input';
import { ClassroomUpdateInput } from './inputs/classroom-update.input';

@Injectable()
export class ClassroomService implements IBaseCrudService<Classroom> {
    constructor(private readonly prisma: PrismaService) { }

    findAll(): Promise<Classroom[]> {
        return this.prisma.classroom.findMany();
    }

    findById(classroomId: string): Promise<Classroom | null> {
        return this.prisma.classroom.findUnique({ where: { id: classroomId } });
    }

    create(data: ClassroomCreateInput): Promise<Classroom> {
        return this.prisma.classroom.create({ data });
    }

    update(classroomId: string, data: ClassroomUpdateInput): Promise<Classroom> {
        return this.prisma.classroom.update({ where: { id: classroomId }, data });
    }

    delete(classroomId: string): Promise<Classroom> {
        return this.prisma.classroom.delete({ where: { id: classroomId } });
    }
}
