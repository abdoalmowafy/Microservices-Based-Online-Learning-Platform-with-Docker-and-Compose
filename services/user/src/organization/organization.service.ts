import { Injectable } from '@nestjs/common';
import { Organization } from '@prisma/client';
import { PrismaService } from 'src/prisma/prisma.service';
import { IBaseCrudService } from 'src/shared/Ibase-crud.service';
import { OrganizationCreateInput } from './inputs/organization-create.input';
import { OrganizationUpdateInput } from './inputs/organization-update.input';

@Injectable()
export class OrganizationService implements IBaseCrudService<Organization> {
    constructor(private readonly prisma: PrismaService) { }

    findAll(): Promise<Organization[]> {
        return this.prisma.organization.findMany();
    }

    findById(organizationId: string): Promise<Organization | null> {
        return this.prisma.organization.findUnique({ where: { id: organizationId } });
    }

    findAllByOwnerId(ownerId: string): Promise<Organization[]> {
        return this.prisma.organization.findMany({ where: { ownerId } });
    }

    findByName(name: string): Promise<Organization | null> {
        return this.prisma.organization.findUnique({ where: { name } });
    }

    create(data: OrganizationCreateInput, ownerId: string): Promise<Organization> {
        return this.prisma.organization.create({ data: { ...data, owner: { connect: { id: ownerId } } } });
    }

    update(organizationId: string, ownerId: string, data: OrganizationUpdateInput): Promise<Organization> {
        return this.prisma.organization.update({ where: { id: organizationId, ownerId }, data });
    }

    logo(organizationId: string, logoUrl: string): Promise<Organization> {
        return this.prisma.organization.update({ where: { id: organizationId }, data: { logoUrl } });
    }

    addModerator(organizationId: string, ownerId: string, moderatorId: string): Promise<Organization> {
        return this.prisma.organization.update({ where: { id: organizationId, ownerId }, data: { moderators: { connect: { id: moderatorId } } } });
    }

    removeModerator(organizationId: string, ownerId: string, userId: string): Promise<Organization> {
        return this.prisma.organization.update({ where: { id: organizationId, ownerId }, data: { moderators: { disconnect: { id: userId } } } });
    }

    delete(organizationId: string, ownerId: string): Promise<Organization> {
        return this.prisma.organization.delete({ where: { id: organizationId, ownerId } });
    }
}
