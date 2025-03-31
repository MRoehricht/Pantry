//
//  Untitled.swift
//  Pantry
//
//  Created by Matthias Röhricht on 30.03.25.
//

import SwiftData
import Foundation

@Model
class Recipe {
    @Attribute var id: UUID?
    var name: String = "kein Name"
    var ingredients: [Ingredient]? = []
    var imageData: Data?
    var tags: [String] = []

    init(name: String? = nil, ingredients: [Ingredient]? = nil,tags: [String]? = nil, imageData: Data? = nil) {
        self.id = UUID()
        self.name = name ?? "kein Name"
        self.ingredients = ingredients ?? []
        self.imageData = imageData
        self.tags = tags ?? []
    }
}

@Model
class Ingredient {
    @Attribute var id: UUID?
    var name: String = "kein Name"
    var quantity: String = "keine Menge"
    @Relationship(inverse: \Recipe.ingredients) var recipes: [Recipe]?

    init(name: String? = nil, quantity: String? = nil) {
        self.id = UUID()
        self.name = name ?? "kein Name"
        self.quantity = quantity ?? "keine Menge"
        self.recipes = []
    }
}
