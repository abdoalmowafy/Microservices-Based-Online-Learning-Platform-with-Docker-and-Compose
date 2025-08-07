import { Args, Mutation, Query, Resolver } from '@nestjs/graphql';
import { Classroom } from './classroom';
import { ClassroomCreateInput } from './inputs/classroom-create.input';
import { ClassroomService } from './classroom.service';

@Resolver(() => Classroom)
export class ClassroomResolver {
    constructor(private readonly classroomService: ClassroomService) { }

    @Query(() => [Classroom])
    async classrooms() {
        return await this.classroomService.findAll();
    }

    @Query(() => Classroom)
    async classroom(@Args('id') id: string) {
        return await this.classroomService.findById(id);
    }

    @Mutation(() => Classroom)
    async createClassroom(@Args('data') data: ClassroomCreateInput) {
        return await this.classroomService.create(data);
    }
}
