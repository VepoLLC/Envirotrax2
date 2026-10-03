import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { PublicSearchComponent } from "./search/public-search.component";

const routes: Routes = [
    {
        path: '',
        title: 'Public Search',
        component: PublicSearchComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PublicSearchRoutingModule {

}
