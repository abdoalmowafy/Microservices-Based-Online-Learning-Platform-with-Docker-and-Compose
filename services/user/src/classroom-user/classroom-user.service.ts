import { Injectable } from '@nestjs/common';
import { IJoinCrudService } from 'src/shared/Ijoin-crud.service';
import { ClassroomUser } from '@prisma/client';
import { ClassroomUserCreateInput } from './inputs/classroom-user-create.input';
import { ClassroomUserUpdateInput } from './inputs/classroom-user-update.input';
import { PrismaService } from 'src/prisma/prisma.service';
import { RoleWithinClassroom } from '@prisma/client';

@Injectable()
export class ClassroomUserService implements IJoinCrudService<ClassroomUser> {
    constructor(private readonly prisma: PrismaService) { }

    findAll(): Promise<ClassroomUser[]> {
        return this.prisma.classroomUser.findMany();
    }
    findById(classroomId: string, userId: string): Promise<ClassroomUser | null> {
        return this.prisma.classroomUser.findUnique({ where: { classroomId_userId: { classroomId, userId } } });
    }
    create(data: ClassroomUserCreateInput & { role: RoleWithinClassroom }): Promise<ClassroomUser> {
        return this.prisma.classroomUser.create({ data: { ...data, roleWithinClassroom: data.role } });
    }
    update(classroomId: string, userId: string, data: ClassroomUserUpdateInput): Promise<ClassroomUser> {
        return this.prisma.classroomUser.update({ where: { classroomId_userId: { classroomId, userId } }, data });
    }
    delete(classroomId: string, userId: string): Promise<ClassroomUser> {
        return this.prisma.classroomUser.update({ where: { classroomId_userId: { classroomId, userId } }, data: { leftAt: new Date() } });
    }
}
