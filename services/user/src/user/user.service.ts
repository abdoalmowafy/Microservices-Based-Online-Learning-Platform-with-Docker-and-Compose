import { Injectable } from '@nestjs/common';
import { IBaseCrudService } from 'src/shared/Ibase-crud.service';
import { User } from '@prisma/client';
import { PrismaService } from 'src/prisma/prisma.service';
import { UserCreateEvent } from './events/incoming/user-create.event';
import { UserUpdateInput } from './inputs/user-update.input';

@Injectable()
export class UserService implements IBaseCrudService<User> {
    constructor(private readonly prisma: PrismaService) { }

    findAll(): Promise<User[]> {
        return this.prisma.user.findMany();
    }

    findById(userId: string): Promise<User | null> {
        return this.prisma.user.findUnique({ where: { id: userId } });
    }

    create(data: UserCreateEvent): Promise<User> {
        return this.prisma.user.create({ data });
    }

    update(userId: string, data: UserUpdateInput): Promise<User> {
        return this.prisma.user.update({ where: { id: userId }, data });
    }

    avatar(userId: string, avatarUrl: string): Promise<User> {
        return this.prisma.user.update({ where: { id: userId }, data: { avatarUrl } });
    }

    delete(userId: string): Promise<User> {
        return this.prisma.user.delete({ where: { id: userId } });
    }
}
