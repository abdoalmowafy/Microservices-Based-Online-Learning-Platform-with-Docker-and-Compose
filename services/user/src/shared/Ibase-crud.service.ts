export interface IBaseCrudService<TModel> {
    findAll(...Args): Promise<TModel[]>;

    findById(...Args): Promise<TModel | null>;

    create(...Args): Promise<TModel>;

    update(...Args): Promise<TModel>;

    delete(...Args): Promise<TModel>;
}